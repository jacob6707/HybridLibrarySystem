import { MutationCache, QueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { meKey } from "@/api/auth";
import { ApiError } from "@/api/client";

export const queryClient: QueryClient = new QueryClient({
	mutationCache: new MutationCache({
		onError: (error) => {
			toast.error(error.message);
			if (error instanceof ApiError && error.status === 401) {
				// Session expired; refresh the current user so the UI shows the sign-in button.
				void queryClient.invalidateQueries({ queryKey: meKey });
			}
		},
	}),
	defaultOptions: {
		queries: {
			refetchOnWindowFocus: false,
			retry: (failureCount, error) =>
				!(error instanceof ApiError && error.status < 500) && failureCount < 2,
		},
	},
});
