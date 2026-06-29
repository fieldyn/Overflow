"use client";

import { Answer } from "@/lib/types";
import { Avatar } from "@heroui/avatar";
import Link from "next/link";
import VotingButtons from "./VotingButtons";

type Props = {
    answer: Answer;
};

export default function AnswerContent({ answer }: Props) {
    return (
        <div className="flex gap-6 px-6 py-6 border-b">
            <VotingButtons votes={0} accepted={answer.accepted} />

            <div className="flex flex-1 flex-col gap-6">
                <div
                    className="prose dark:prose-invert max-w-none"
                    dangerouslySetInnerHTML={{ __html: answer.content }}
                />

                <div className="flex justify-end items-end pt-2">
                    <div className="flex flex-col gap-1 rounded bg-default-100 p-3 text-sm">
                        <span className="text-default-500">
                            answered {answer.createdAt}
                        </span>
                        <div className="flex items-center gap-2">
                            <Avatar
                                className="h-6 w-6"
                                color="secondary"
                                name={answer.userDisplayName.charAt(0)}
                            />
                            <Link
                                href={`/profiles/${answer.userId}`}
                                className="text-primary hover:underline"
                            >
                                {answer.userDisplayName}
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
