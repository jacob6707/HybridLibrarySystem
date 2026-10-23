import { useEffect, useState } from "react";

interface Author {
	id: number;
	name: string;
	biography: string;
	dateOfBirth: string;
	dateOfDeath: string;
	nationality: string;
}

interface Category {
	id: number;
	name: string;
}

interface Book {
	id: number;
	title: string;
	isbn: string;
	author: Author;
	category: Category;
	publishedDate: string;
	copiesAvailable: number;
}

// 1. Helper function to extract the readable cookie
const getCookie = (name: string) => {
	const value = `; ${document.cookie}`;
	console.log(value);
	const parts = value.split(`; ${name}=`);
	console.log(parts[1]);
	// console.log(parts.pop()?.split(";").shift());
	if (parts.length === 2) return parts.pop()?.split(";").shift();
	return null;
};

// 2. The test request
const testXsrfPost = async () => {
	// Grab the token the middleware placed in the readable cookie
	const xsrfToken = getCookie("XSRF-TOKEN");

	console.log("XSRF-TOKEN cookie value:", xsrfToken);

	if (!xsrfToken) {
		console.error(
			"No XSRF-TOKEN cookie found! Did you make a GET request first?",
		);
		return;
	}

	try {
		const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/books`, {
			// Replace with your actual POST endpoint
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				// Attach the decoded token to the exact header ASP.NET expects
				"X-XSRF-TOKEN": decodeURIComponent(xsrfToken),
			},
			// CRITICAL: Forces the browser to send the HttpOnly security cookies alongside the header
			credentials: "include",
			body: JSON.stringify({
				title: "Test Title",
			}),
		});

		if (response.ok) {
			console.log("✅ Success! Antiforgery token was accepted.");
			const data = await response.json();
			console.log(data);
		} else {
			console.error(`❌ Failed with status: ${response.status}`);
		}
	} catch (error) {
		console.error("Network or CORS error:", error);
	}
};

function App() {
	const [books, setBooks] = useState([] as Book[]);
	useEffect(() => {
		fetch(`${import.meta.env.VITE_API_BASE_URL}/books`).then(
			async (response) => {
				const data = await response.json();
				console.log(data);
				setBooks(data);
			},
		);
	}, []);
	return (
		<>
			<h1>Books</h1>
			<ul>
				{books.map((book) => (
					<li key={book.id}>
						{book.title} ({new Date(book.publishedDate).getFullYear()},{" "}
						{book.category.name}) - {book.author.name}
					</li>
				))}
			</ul>
			<button onClick={testXsrfPost}>Test XSRF POST</button>
		</>
	);
}

export default App;
