import { useQuery } from "@tanstack/react-query";
import { apiFetch, ApiError } from "@/api/client";
import type { CurrentUser } from "@/api/types";
import type { Permission } from "@/lib/permissions";

export const meKey = ["me"] as const;

export function useMe() {
	return useQuery({
		queryKey: meKey,
		staleTime: 5 * 60 * 1000,
		queryFn: async () => {
			try {
				return await apiFetch<CurrentUser>("/api/v1/auth/me");
			} catch (error) {
				// Anonymous visitors are a normal state, not an error.
				if (error instanceof ApiError && error.status === 401) return null;
				throw error;
			}
		},
	});
}

export function hasPermission(
	me: CurrentUser | null | undefined,
	permission: Permission,
) {
	return me?.permissions.includes(permission) ?? false;
}

export function redirectToLogin() {
	const returnUrl = window.location.pathname + window.location.search;
	window.location.assign(
		`/Identity/Account/Login?ReturnUrl=${encodeURIComponent(returnUrl)}`,
	);
}

export async function logout() {
	await apiFetch(`/Identity/Account/Logout?returnUrl=${encodeURIComponent("/app/")}`, {
		method: "POST",
	}).catch(() => undefined);
	window.location.assign("/app/");
}
