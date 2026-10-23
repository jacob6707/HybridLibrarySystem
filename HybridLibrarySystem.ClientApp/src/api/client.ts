export class ApiError extends Error {
	status: number;

	constructor(status: number, message: string) {
		super(message);
		this.name = "ApiError";
		this.status = status;
	}
}

interface ProblemDetails {
	title?: string;
	detail?: string;
	errors?: Record<string, string[]>;
}

const getCookie = (name: string) => {
	const prefix = `${name}=`;
	const entry = document.cookie
		.split("; ")
		.find((cookie) => cookie.startsWith(prefix));
	return entry ? decodeURIComponent(entry.slice(prefix.length)) : null;
};

const defaultMessage = (status: number) => {
	if (status === 401) return "Please sign in to continue.";
	if (status === 403) return "You don't have permission to do that.";
	return `Request failed (${status}).`;
};

async function toApiError(response: Response) {
	let message = defaultMessage(response.status);
	try {
		const problem = (await response.json()) as ProblemDetails;
		const validation = Object.values(problem.errors ?? {}).flat();
		if (validation.length > 0) {
			message = validation.join(" ");
		} else if (problem.detail ?? problem.title) {
			message = (problem.detail ?? problem.title)!;
		}
	} catch {
		// Response had no JSON body; keep the default message.
	}
	return new ApiError(response.status, message);
}

export async function apiFetch<T = void>(
	path: string,
	init: Omit<RequestInit, "body"> & { body?: unknown } = {},
): Promise<T> {
	const method = (init.method ?? "GET").toUpperCase();
	const headers = new Headers(init.headers);
	headers.set("Accept", "application/json");

	if (init.body !== undefined) {
		headers.set("Content-Type", "application/json");
	}
	if (method !== "GET" && method !== "HEAD") {
		const token = getCookie("XSRF-TOKEN");
		if (token) headers.set("X-XSRF-TOKEN", token);
	}

	const response = await fetch(path, {
		...init,
		method,
		headers,
		credentials: "same-origin",
		body: init.body === undefined ? undefined : JSON.stringify(init.body),
	});

	if (!response.ok) {
		throw await toApiError(response);
	}
	if (response.status === 204) {
		return undefined as T;
	}
	return (await response.json()) as T;
}
