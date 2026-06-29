"use client";

import { Question } from "@/lib/types"
import { Chip } from "@heroui/chip";
import { Avatar } from "@heroui/avatar";
import Link from "next/link";
import clsx from "clsx";
import { CheckIcon } from "@heroicons/react/24/outline";

type Props = {
    question: Question;
}

export default function QuestionCard({ question }: Props) {
    return (
        <div className="flex gap-6 px-6">
            <div className="flex flex-col text-sm gap-3 min-w-[6rem]">
                <div>
                    <span>{question.votes}</span> {question.votes === 1 ? 'vote' : 'votes'}
                </div>
                <div>
                    <span className={clsx('inline-flex items-center gap-1 rounded px-2 py-1', {
                        'border border-success text-success': question.answersCount > 0 && !question.hasAcceptedAnswer,
                        'border border-success bg-success text-white': question.hasAcceptedAnswer,
                    })}>
                        {question.hasAcceptedAnswer && <CheckIcon className="h-4 w-4" strokeWidth={3} />}
                        {question.answersCount} {question.answersCount === 1 ? 'answer' : 'answers'}
                    </span>
                </div>
                <div>
                    <span>{question.viewCount}</span> {question.viewCount === 1 ? 'view' : 'views'}
                </div>
            </div>
            <div className="flex flex-1 justify-between min-h-[8rem]">
                <div className="flex flex-col gap-2">
                    <Link href={`/questions/${question.id}`}
                        className='text-primary font-semibold hover:underline first-letter:uppercase'>
                        {question.title}
                    </Link>
                    <div
                        className="line-clamp-2"
                        dangerouslySetInnerHTML={{__html: question.content}}
                    />
                    <div className="flex justify-between items-end pt-2">
                        <div className="flex gap-2">
                            {question.tagSlugs.map(slug => (
                                <Chip
                                    key={slug}
                                    variant="bordered"
                                    as={Link}
                                    href={`/questions?tag=${slug}`}
                                >{slug}</Chip>
                            ))}
                        </div>
                        <div className="flex flex-col gap-1 rounded bg-default-100 p-2 text-sm">
                            <span className="text-default-500">asked {question.createdAt}</span>
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
        </div>
    )
}
