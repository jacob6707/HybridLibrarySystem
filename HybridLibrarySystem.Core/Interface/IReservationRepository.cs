namespace HybridLibrarySystem.Core.Interface;

public enum ReserveResult
{
    Reserved,
    BookNotFound,
    AlreadyReserved,
    NoCopiesAvailable
}

public interface IReservationRepository
{
    Task<ReserveResult> ReserveAsync(int bookId, string userId);
    Task<bool> UnreserveAsync(int bookId, string userId);
    Task<HashSet<int>> GetReservedBookIdsAsync(string userId);
    Task<bool> BookHasReservationsAsync(int bookId);
}
