"use client";

import { Question } from "@/lib/types";
import { Button } from "@heroui/button";
import Link from "next/link";

type Props = {
    question: Question;
};

export default function QuestionDetailedHeader({ question }: Props) {
    return (
        <div className="flex flex-col w-full border-b gap-4 pb-4 px-6">
            <div className="flex justify-between items-center gap-4 w-full">
                <h1 className="text-2xl font-semibold first-letter:uppercase">
                    {question.title}
                </h1>
                <Button as={Link} href="/questions/ask" color="secondary" className="shrink-0">
                    Ask Question
                </Button>
            </div>
            <div className="flex items-center gap-6 text-sm text-default-500">
                <span>
                    Asked <span className="text-default-700">{question.createdAt}</span>
                </span>
                {question.updatedAt && (
                    <span>
                        Modified <span className="text-default-700">{question.updatedAt}</span>
                    </span>
                )}
                <span>
                    Viewed{" "}
                    <span className="text-default-700">
                        {question.viewCount} {question.viewCount === 1 ? "time" : "times"}
                    </span>
                </span>
            </div>
        </div>
    );
}