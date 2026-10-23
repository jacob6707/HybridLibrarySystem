namespace HybridLibrarySystem.Core.Model.Authorization;

public static class Permissions
{
    public const string CreateAuthor = "author:create";
    public const string ReadAuthor = "author:read";
    public const string UpdateAuthor = "author:update";
    public const string DeleteAuthor = "author:delete";

    public const string CreateBook = "book:create";
    public const string ReadBook = "book:read";
    public const string UpdateBook = "book:update";
    public const string DeleteBook = "book:delete";
    public const string ReserveBook = "book:reserve";

    public const string CreateMember = "member:create";
    public const string ReadMember = "member:read";
    public const string UpdateMember = "member:update";
    public const string DeleteMember = "member:delete";

    public const string CreateCategory = "category:create";
    public const string ReadCategory = "category:read";
    public const string UpdateCategory = "category:update";
    public const string DeleteCategory = "category:delete";

    public static IEnumerable<string> GetPermissions() => typeof(Permissions).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string))
        .Select(f => (string)f.GetValue(null)!);
}
