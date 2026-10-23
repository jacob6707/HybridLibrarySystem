import { useState } from "react";
import { Trash2 } from "lucide-react";
import { useDeleteBook } from "@/api/books";
import type { Book } from "@/api/types";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";

export function DeleteBookDialog({ book }: { book: Book }) {
	const [open, setOpen] = useState(false);
	const deleteBook = useDeleteBook();

	return (
		<>
			<Button variant="destructive" size="sm" onClick={() => setOpen(true)}>
				<Trash2 />
				Delete
			</Button>
			<AlertDialog open={open} onOpenChange={setOpen}>
				<AlertDialogContent>
					<AlertDialogHeader>
						<AlertDialogTitle>Delete “{book.title}”?</AlertDialogTitle>
						<AlertDialogDescription>
							This cannot be undone. Books with active reservations cannot be
							deleted.
						</AlertDialogDescription>
					</AlertDialogHeader>
					<AlertDialogFooter>
						<AlertDialogCancel>Cancel</AlertDialogCancel>
						<AlertDialogAction
							variant="destructive"
							disabled={deleteBook.isPending}
							onClick={() =>
								deleteBook.mutate(book.id, { onSettled: () => setOpen(false) })
							}
						>
							Delete
						</AlertDialogAction>
					</AlertDialogFooter>
				</AlertDialogContent>
			</AlertDialog>
		</>
	);
}
