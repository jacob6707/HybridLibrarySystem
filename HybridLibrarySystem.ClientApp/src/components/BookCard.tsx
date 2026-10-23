import { Link } from "react-router";
import { Pencil } from "lucide-react";
import { hasPermission, redirectToLogin, useMe } from "@/api/auth";
import { useReserveBook, useUnreserveBook } from "@/api/books";
import type { Book } from "@/api/types";
import { DeleteBookDialog } from "@/components/DeleteBookDialog";
import { Badge } from "@/components/ui/badge";
import { Button, buttonVariants } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import { Permissions } from "@/lib/permissions";

export function BookCard({ book }: { book: Book }) {
	const { data: me } = useMe();
	const reserve = useReserveBook();
	const unreserve = useUnreserveBook();

	const canReserve = hasPermission(me, Permissions.ReserveBook);
	const canEdit = hasPermission(me, Permissions.UpdateBook);
	const canDelete = hasPermission(me, Permissions.DeleteBook);
	const inStock = book.copiesAvailable > 0;
	const busy = reserve.isPending || unreserve.isPending;

	return (
		<Card>
			<CardHeader>
				<CardTitle>{book.title}</CardTitle>
				<CardDescription>{book.author.name}</CardDescription>
			</CardHeader>
			<CardContent className="flex flex-col gap-3">
				<div className="flex flex-wrap items-center gap-2">
					<Badge variant="secondary">{book.category.name}</Badge>
					<Badge variant={inStock ? "outline" : "destructive"}>
						{inStock
							? `${book.copiesAvailable} available`
							: "Out of stock"}
					</Badge>
					{book.isReservedByMe && <Badge>Reserved by you</Badge>}
				</div>
				<dl className="grid grid-cols-[auto_1fr] gap-x-3 gap-y-1 text-muted-foreground">
					<dt>Published</dt>
					<dd>{new Date(book.publishedDate).getFullYear()}</dd>
					<dt>ISBN</dt>
					<dd>{book.isbn}</dd>
				</dl>
			</CardContent>
			<CardFooter className="flex-wrap gap-2">
				{me === null && (
					<Button size="sm" variant="outline" onClick={redirectToLogin}>
						Sign in to reserve
					</Button>
				)}
				{canReserve &&
					(book.isReservedByMe ? (
						<Button
							size="sm"
							variant="outline"
							disabled={busy}
							onClick={() => unreserve.mutate(book.id)}
						>
							Unreserve
						</Button>
					) : (
						<Button
							size="sm"
							disabled={busy || !inStock}
							onClick={() => reserve.mutate(book.id)}
						>
							Reserve
						</Button>
					))}
				{canEdit && (
					<Link
						to={`/books/${book.id}/edit`}
						className={buttonVariants({ variant: "outline", size: "sm" })}
					>
						<Pencil />
						Edit
					</Link>
				)}
				{canDelete && <DeleteBookDialog book={book} />}
			</CardFooter>
		</Card>
	);
}
