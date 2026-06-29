"use client";

import { Question } from "@/lib/types";
import VotingButtons from "./VotingButtons";
import QuestionFooter from "./QuestionFooter";

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

                <QuestionFooter question={question} />
            </div>
        </div>
    );
}