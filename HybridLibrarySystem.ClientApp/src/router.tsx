import { createBrowserRouter } from "react-router";
import { Layout } from "@/components/Layout";
import { BooksPage } from "@/pages/BooksPage";
import { EditBookPage } from "@/pages/EditBookPage";

export const router = createBrowserRouter(
	[
		{
			element: <Layout />,
			children: [
				{ index: true, element: <BooksPage /> },
				{ path: "books/:id/edit", element: <EditBookPage /> },
			],
		},
	],
	{ basename: "/app" },
);
