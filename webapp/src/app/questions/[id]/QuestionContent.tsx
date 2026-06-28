"use client";

import { Question } from "@/lib/types";
import { Chip } from "@heroui/chip";
import { Avatar } from "@heroui/avatar";
import Link from "next/link";
import VotingButtons from "./VotingButtons";

type Props = {
    question: Question;
};

export default function QuestionContent({ question }: Props) {
    return (
        <div className="flex gap-6 px-6 py-6 border-b">
            <VotingButtons votes={question.votes} />

            <div className="flex flex-1 flex-col gap-6">
                <div
                    className="prose dark:prose-invert max-w-none"
                    dangerouslySetInnerHTML={{ __html: question.content }}
                />

                <div className="flex justify-between items-end pt-2">
                    <div className="flex gap-2">
                        {question.tagSlugs.map((slug) => (
                            <Chip
                                key={slug}
                                variant="bordered"
                                as={Link}
                                href={`/questions?tag=${slug}`}
                            >
                                {slug}
                            </Chip>
                        ))}
                    </div>

                    <div className="flex flex-col gap-1 rounded bg-default-100 p-3 text-sm">
                        <span className="text-default-500">
                            asked {question.createdAt}
                        </span>
                        <div className="flex items-center gap-2">
                            <Avatar
                                className="h-6 w-6"
                                color="secondary"
                                name={question.askerDisplayName.charAt(0)}
                            />
                            <Link
                                href={`/profiles/${question.askerId}`}
                                className="text-primary hover:underline"
                            >
                                {question.askerDisplayName}
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}