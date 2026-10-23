import { Outlet } from "react-router";
import { logout, redirectToLogin, useMe } from "@/api/auth";
import { Button } from "@/components/ui/button";
import { Toaster } from "@/components/ui/sonner";

export function Layout() {
	const { data: me, isPending } = useMe();

	return (
		<div className="min-h-screen bg-background text-foreground">
			<header className="border-b">
				<div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
					<a href="/" className="text-lg font-semibold">
						Hybrid Library
					</a>
					{!isPending &&
						(me ? (
							<div className="flex items-center gap-3 text-sm">
								<span className="text-muted-foreground">
									Signed in as {me.username}
								</span>
								<Button variant="outline" size="sm" onClick={() => void logout()}>
									Sign out
								</Button>
							</div>
						) : (
							<Button size="sm" onClick={redirectToLogin}>
								Sign in
							</Button>
						))}
				</div>
			</header>
			<main className="mx-auto max-w-6xl px-4 py-6">
				<Outlet />
			</main>
			<Toaster />
		</div>
	);
}
