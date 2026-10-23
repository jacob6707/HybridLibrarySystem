using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace HybridLibrarySystem.Data.Repository;

internal sealed class ReservationRepository(LibraryContext db) : IReservationRepository
{
    public async Task<ReserveResult> ReserveAsync(int bookId, string userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        if (!await db.Books.AnyAsync(b => b.Id == bookId))
        {
            return ReserveResult.BookNotFound;
        }

        if (await db.Reservations.AnyAsync(r => r.BookId == bookId && r.UserId == userId))
        {
            return ReserveResult.AlreadyReserved;
        }

        // Conditional decrement so concurrent requests can never push copies below zero.
        var updated = await db.Books
            .Where(b => b.Id == bookId && b.CopiesAvailable > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.CopiesAvailable, b => b.CopiesAvailable - 1));
        if (updated == 0)
        {
            return ReserveResult.NoCopiesAvailable;
        }

        db.Reservations.Add(new Reservation
        {
            BookId = bookId,
            UserId = userId,
            ReservedAt = DateTime.UtcNow
        });

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Unique index (BookId, UserId) hit by a concurrent request; the transaction rolls back the decrement.
            return ReserveResult.AlreadyReserved;
        }

        await transaction.CommitAsync();
        return ReserveResult.Reserved;
    }

    public async Task<bool> UnreserveAsync(int bookId, string userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        var deleted = await db.Reservations
            .Where(r => r.BookId == bookId && r.UserId == userId)
            .ExecuteDeleteAsync();
        if (deleted == 0)
        {
            return false;
        }

        await db.Books
            .Where(b => b.Id == bookId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.CopiesAvailable, b => b.CopiesAvailable + 1));

        await transaction.CommitAsync();
        return true;
    }

    public async Task<HashSet<int>> GetReservedBookIdsAsync(string userId)
    {
        var ids = await db.Reservations
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => r.BookId)
            .ToListAsync();
        return ids.ToHashSet();
    }

    public Task<bool> BookHasReservationsAsync(int bookId)
    {
        return db.Reservations.AnyAsync(r => r.BookId == bookId);
    }
}
