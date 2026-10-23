import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { apiFetch } from "@/api/client";
import type { Book, UpdateBookInput } from "@/api/types";

export const booksKey = ["books"] as const;

export function useBooks() {
	return useQuery({
		queryKey: booksKey,
		queryFn: () => apiFetch<Book[]>("/api/v1/books"),
	});
}

export function useBook(id: number) {
	return useQuery({
		queryKey: [...booksKey, id],
		queryFn: () => apiFetch<Book>(`/api/v1/books/${id}`),
		enabled: Number.isInteger(id),
	});
}

function useBooksMutation<TVariables>(
	mutationFn: (variables: TVariables) => Promise<unknown>,
	successMessage: string,
) {
	const queryClient = useQueryClient();
	return useMutation({
		mutationFn,
		onSuccess: async () => {
			toast.success(successMessage);
			await queryClient.invalidateQueries({ queryKey: booksKey });
		},
	});
}

export function useReserveBook() {
	return useBooksMutation(
		(id: number) =>
			apiFetch(`/api/v1/books/${id}/reservation`, { method: "POST" }),
		"Book reserved.",
	);
}

export function useUnreserveBook() {
	return useBooksMutation(
		(id: number) =>
			apiFetch(`/api/v1/books/${id}/reservation`, { method: "DELETE" }),
		"Reservation removed.",
	);
}

export function useUpdateBook() {
	return useBooksMutation(
		({ id, input }: { id: number; input: UpdateBookInput }) =>
			apiFetch<Book>(`/api/v1/books/${id}`, { method: "PUT", body: input }),
		"Book updated.",
	);
}

export function useDeleteBook() {
	return useBooksMutation(
		(id: number) => apiFetch(`/api/v1/books/${id}`, { method: "DELETE" }),
		"Book deleted.",
	);
}
