'use client'
import { AcademicCapIcon } from "@heroicons/react/24/solid";
import { Button } from "@heroui/button";
import Link from "next/link";

export default function Home() {
  return (
    <div className="flex items-center h-[calc(100vh-160px)] justify-center">
      <div className="flex flex-col items-center text-center max-w-xl gap-6 px-6">
        <AcademicCapIcon className="h-24 w-24 text-primary" />

        <h1 className="text-4xl font-bold tracking-tight">
          Welcome to Overflow
        </h1>

        <p className="text-lg text-default-500">
          Ask questions, share what you know, and learn together with the
          community. Your next answer is just a search away.
        </p>

        <div className="flex gap-3">
          <Button as={Link} href="/questions" color="primary" size="lg">
            Browse Questions
          </Button>
          <Button
            as={Link}
            href="/questions/ask"
            color="secondary"
            variant="bordered"
            size="lg"
          >
            Ask a Question
          </Button>
        </div>
      </div>
    </div>
  );
}
