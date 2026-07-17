"use client";

import { Tag } from "@/lib/types";
import { Card, CardBody, CardHeader } from "@heroui/card";
import Link from "next/link";

type Props = {
    tag: Tag;
}

export default function TagCard({ tag }: Props) {
    return (
        <Card
            as={Link}
            href={`/questions?tag=${tag.slug}`}
            isPressable
        >
            <CardHeader className="font-semibold text-primary">
                {tag.name}
            </CardHeader>
            <CardBody className="pt-0">
                <p className="text-sm text-default-500 line-clamp-3">
                    {tag.description}
                </p>
            </CardBody>
        </Card>
    )
}
