import { useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { hasPermission, redirectToLogin, useMe } from "@/api/auth";
import { useBook, useUpdateBook } from "@/api/books";
import { ApiError } from "@/api/client";
import { useAuthors, useCategories } from "@/api/lookups";
import type { Author, Book, Category } from "@/api/types";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Permissions } from "@/lib/permissions";

function Notice({ children }: { children: React.ReactNode }) {
	return (
		<div className="flex flex-col items-start gap-3">
			{children}
			<Link to="/" className={buttonVariants({ variant: "outline", size: "sm" })}>
				Back to books
			</Link>
		</div>
	);
}

export function EditBookPage() {
	const params = useParams();
	const bookId = Number(params.id);
	const { data: me, isPending: meLoading } = useMe();
	const canEdit = hasPermission(me, Permissions.UpdateBook);
	const book = useBook(bookId);
	const authors = useAuthors(canEdit);
	const categories = useCategories(canEdit);

	if (!Number.isInteger(bookId)) {
		return <Notice>Book not found.</Notice>;
	}
	if (meLoading) {
		return <Skeleton className="h-96 max-w-xl rounded-xl" />;
	}
	if (me === null) {
		return (
			<Notice>
				<p>You need to sign in to edit books.</p>
				<Button size="sm" onClick={redirectToLogin}>
					Sign in
				</Button>
			</Notice>
		);
	}
	if (!canEdit) {
		return <Notice>You don't have permission to edit books.</Notice>;
	}
	if (book.isError) {
		const notFound = book.error instanceof ApiError && book.error.status === 404;
		return <Notice>{notFound ? "Book not found." : book.error.message}</Notice>;
	}
	if (book.isPending || authors.isPending || categories.isPending) {
		return <Skeleton className="h-96 max-w-xl rounded-xl" />;
	}
	if (authors.isError || categories.isError) {
		return <Notice>Could not load authors and categories.</Notice>;
	}

	return (
		<EditBookForm
			book={book.data}
			authors={authors.data}
			categories={categories.data}
		/>
	);
}

function EditBookForm({
	book,
	authors,
	categories,
}: {
	book: Book;
	authors: Author[];
	categories: Category[];
}) {
	const navigate = useNavigate();
	const updateBook = useUpdateBook();
	const [title, setTitle] = useState(book.title);
	const [isbn, setIsbn] = useState(book.isbn);
	const [authorId, setAuthorId] = useState(String(book.author.id));
	const [categoryId, setCategoryId] = useState(String(book.category.id));
	const [publishedDate, setPublishedDate] = useState(
		book.publishedDate.slice(0, 10),
	);
	const [copies, setCopies] = useState(String(book.copiesAvailable));

	const authorItems = authors.map((a) => ({ value: String(a.id), label: a.name }));
	const categoryItems = categories.map((c) => ({
		value: String(c.id),
		label: c.name,
	}));

	const onSubmit = (event: FormEvent) => {
		event.preventDefault();
		updateBook.mutate(
			{
				id: book.id,
				input: {
					title,
					isbn,
					authorId: Number(authorId),
					categoryId: Number(categoryId),
					publishedDate,
					copiesAvailable: Number(copies),
				},
			},
			{ onSuccess: () => void navigate("/") },
		);
	};

	return (
		<Card className="max-w-xl">
			<CardHeader>
				<CardTitle>Edit book</CardTitle>
			</CardHeader>
			<CardContent>
				<form onSubmit={onSubmit} className="flex flex-col gap-4">
					<div className="flex flex-col gap-1.5">
						<Label htmlFor="title">Title</Label>
						<Input
							id="title"
							value={title}
							maxLength={200}
							required
							onChange={(e) => setTitle(e.target.value)}
						/>
					</div>
					<div className="flex flex-col gap-1.5">
						<Label htmlFor="isbn">ISBN</Label>
						<Input
							id="isbn"
							value={isbn}
							maxLength={13}
							required
							onChange={(e) => setIsbn(e.target.value)}
						/>
					</div>
					<div className="flex flex-col gap-1.5">
						<Label>Author</Label>
						<Select
							items={authorItems}
							value={authorId}
							onValueChange={(value) => value && setAuthorId(value)}
						>
							<SelectTrigger className="w-full">
								<SelectValue />
							</SelectTrigger>
							<SelectContent>
								{authorItems.map((item) => (
									<SelectItem key={item.value} value={item.value}>
										{item.label}
									</SelectItem>
								))}
							</SelectContent>
						</Select>
					</div>
					<div className="flex flex-col gap-1.5">
						<Label>Category</Label>
						<Select
							items={categoryItems}
							value={categoryId}
							onValueChange={(value) => value && setCategoryId(value)}
						>
							<SelectTrigger className="w-full">
								<SelectValue />
							</SelectTrigger>
							<SelectContent>
								{categoryItems.map((item) => (
									<SelectItem key={item.value} value={item.value}>
										{item.label}
									</SelectItem>
								))}
							</SelectContent>
						</Select>
					</div>
					<div className="flex flex-col gap-1.5">
						<Label htmlFor="publishedDate">Date published</Label>
						<Input
							id="publishedDate"
							type="date"
							value={publishedDate}
							required
							onChange={(e) => setPublishedDate(e.target.value)}
						/>
					</div>
					<div className="flex flex-col gap-1.5">
						<Label htmlFor="copies">Copies available</Label>
						<Input
							id="copies"
							type="number"
							min={0}
							step={1}
							value={copies}
							required
							onChange={(e) => setCopies(e.target.value)}
						/>
					</div>
					<div className="flex gap-2">
						<Button type="submit" disabled={updateBook.isPending}>
							Save changes
						</Button>
						<Link to="/" className={buttonVariants({ variant: "outline" })}>
							Cancel
						</Link>
					</div>
				</form>
			</CardContent>
		</Card>
	);
}
