"use client";

import { Button } from "@heroui/button";
import {
    ChevronUpIcon,
    ChevronDownIcon,
    CheckIcon,
} from "@heroicons/react/24/outline";
import clsx from "clsx";
import { useState } from "react";

type Props = {
    votes: number;
    accepted?: boolean;
};

export default function VotingButtons({ votes, accepted }: Props) {
    // -1 = downvoted, 0 = none, 1 = upvoted
    const [vote, setVote] = useState<-1 | 0 | 1>(0);

    const toggle = (value: 1 | -1) => setVote((prev) => (prev === value ? 0 : value));

    return (
        <div className="flex flex-col items-center gap-1 text-default-500">
            <Button
                isIconOnly
                variant="light"
                radius="full"
                aria-label="Vote up"
                onPress={() => toggle(1)}
                className={clsx(vote === 1 && "text-primary")}
            >
                <ChevronUpIcon className="h-6 w-6" />
            </Button>

            <span className="text-xl font-semibold text-default-700">
                {votes + vote}
            </span>

            <Button
                isIconOnly
                variant="light"
                radius="full"
                aria-label="Vote down"
                onPress={() => toggle(-1)}
                className={clsx(vote === -1 && "text-danger")}
            >
                <ChevronDownIcon className="h-6 w-6" />
            </Button>

            {accepted && (
                <span
                    className="mt-1 rounded-full bg-success p-1 text-white"
                    title="Accepted answer"
                >
                    <CheckIcon className="h-5 w-5" strokeWidth={3} />
                </span>
            )}
        </div>
    );
}
