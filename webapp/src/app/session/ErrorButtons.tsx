'use client';

import { useState, useTransition } from "react";
import { Button } from "@heroui/button";
import { getError } from "@/lib/actions/error.actions";
import { handleError } from "@/lib/utils";

const errorCodes = [
  { code: 400, label: "Bad Request", color: "warning" },
  { code: 401, label: "Unauthorized", color: "secondary" },
  { code: 404, label: "Not Found", color: "default" },
  { code: 500, label: "Server Error", color: "danger" },
] as const;

export default function ErrorButtons() {
  const [pending, startTransition] = useTransition();
  const [target, setTarget] = useState<number | null>(null);

  const triggerError = (code: number) => {
    setTarget(code);
    // Throwing inside startTransition bubbles up to the nearest error boundary
    startTransition(async () => {
      const { error } = await getError(code);
      if (error) handleError(error);
      setTarget(0);
    });
  };

  return (
    <div className="flex flex-wrap gap-3">
      {errorCodes.map(({ code, label, color }) => (
        <Button
          key={code}
          color={color}
          variant="bordered"
          isLoading={pending && target === code}
          onPress={() => triggerError(code)}
        >
          {label} ({code})
        </Button>
      ))}
    </div>
  );
}
