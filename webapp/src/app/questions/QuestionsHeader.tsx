"use client";

import clsx from "clsx";
import { Button } from "@heroui/button";
import Link from "next/link";

const tabs = [
    { key: "newest", label: "Newest" },
    { key: "active", label: "Active" },
    { key: "unanswered", label: "Unanswered" },
];

type Props = {
    tag?: string;
    total: number;
};

export default function QuestionsHeader({ tag, total }: Props) {
    const activeTab = "newest";

    return (
        <div className="flex flex-col w-full border-b gap-4 pb-4 px-6">
            <div className="flex justify-between items-center w-full">
                <h1 className="text-2xl font-semibold first-letter:uppercase">
                    {tag ? `${tag} Questions` : "All Questions"}
                </h1>
                <Button as={Link} href="/questions/ask" color="secondary">
                    Ask Question
                </Button>
            </div>
            <div className="flex justify-between items-center w-full">
                <span className="text-sm text-default-500">
                    {total} {total === 1 ? "question" : "questions"}
                </span>
                <div className="inline-flex gap-1 rounded-full bg-default-100 p-1">
                    {tabs.map((tab) => (
                        <div
                            key={tab.key}
                            className={clsx(
                                "cursor-pointer rounded-full px-4 py-1.5 text-sm transition-colors",
                                tab.key === activeTab
                                    ? "bg-primary text-white font-medium shadow-sm"
                                    : "text-default-500 hover:text-default-700"
                            )}
                        >
                            {tab.label}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
