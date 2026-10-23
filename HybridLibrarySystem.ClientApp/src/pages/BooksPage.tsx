import { useBooks } from "@/api/books";
import { BookCard } from "@/components/BookCard";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";

export function BooksPage() {
	const { data: books, isPending, isError, error, refetch } = useBooks();

	return (
		<section className="flex flex-col gap-4">
			<h1 className="text-2xl font-semibold">Books</h1>
			{isPending && (
				<div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
					{Array.from({ length: 6 }, (_, i) => (
						<Skeleton key={i} className="h-48 rounded-xl" />
					))}
				</div>
			)}
			{isError && (
				<div className="flex items-center gap-3">
					<p className="text-destructive">{error.message}</p>
					<Button variant="outline" size="sm" onClick={() => void refetch()}>
						Retry
					</Button>
				</div>
			)}
			{books?.length === 0 && (
				<p className="text-muted-foreground">There are no books yet.</p>
			)}
			{books && books.length > 0 && (
				<div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
					{books.map((book) => (
						<BookCard key={book.id} book={book} />
					))}
				</div>
			)}
		</section>
	);
}
