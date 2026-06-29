"use client";

import { Answer } from "@/lib/types";
import VotingButtons from "./VotingButtons";
import AnswerFooter from "./AnswerFooter";

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

                <AnswerFooter answer={answer} />
            </div>
        </div>
    );
}
