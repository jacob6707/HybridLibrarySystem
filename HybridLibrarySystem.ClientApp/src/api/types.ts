export interface Author {
	id: number;
	name: string;
	biography: string;
	dateOfBirth: string;
	dateOfDeath: string | null;
	nationality: string;
}

export interface Category {
	id: number;
	name: string;
}

export interface Book {
	id: number;
	title: string;
	isbn: string;
	author: Author;
	category: Category;
	publishedDate: string;
	copiesAvailable: number;
	isReservedByMe: boolean;
}

export interface UpdateBookInput {
	title: string;
	isbn: string;
	authorId: number;
	categoryId: number;
	publishedDate: string;
	copiesAvailable: number;
}

export interface CurrentUser {
	userId: string;
	username: string;
	roles: string[];
	permissions: string[];
}
