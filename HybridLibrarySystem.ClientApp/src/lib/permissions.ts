// Mirrors HybridLibrarySystem.Core.Model.Authorization.Permissions. Only used to show or hide UI; the server enforces access.
export const Permissions = {
	ReadAuthor: "author:read",
	ReadCategory: "category:read",
	UpdateBook: "book:update",
	DeleteBook: "book:delete",
	ReserveBook: "book:reserve",
} as const;

export type Permission = (typeof Permissions)[keyof typeof Permissions];
