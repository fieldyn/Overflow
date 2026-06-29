"use client";

import { Select, SelectItem } from "@heroui/select";

const sortOptions = [
    { key: "score", label: "Highest score (default)" },
    { key: "created", label: "Date created" },
];

type Props = {
    answerCount: number;
};

export default function AnswersHeader({ answerCount }: Props) {
    return (
        <div className="flex justify-between items-center w-full border-b gap-4 py-4 px-6">
            <h2 className="text-xl font-semibold">
                {answerCount} {answerCount === 1 ? "Answer" : "Answers"}
            </h2>
            <Select
                aria-label="Sort answers"
                size="sm"
                className="max-w-56"
                defaultSelectedKeys={["score"]}
            >
                {sortOptions.map((option) => (
                    <SelectItem key={option.key}>{option.label}</SelectItem>
                ))}
            </Select>
        </div>
    );
}
