import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@/api/client";
import type { Author, Category } from "@/api/types";

export function useAuthors(enabled = true) {
	return useQuery({
		queryKey: ["authors"],
		queryFn: () => apiFetch<Author[]>("/api/v1/authors"),
		enabled,
	});
}

export function useCategories(enabled = true) {
	return useQuery({
		queryKey: ["categories"],
		queryFn: () => apiFetch<Category[]>("/api/v1/categories"),
		enabled,
	});
}
