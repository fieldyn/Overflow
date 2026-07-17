'use client' // Error boundaries must be Client Components

import { useEffect } from 'react'
import { Button } from '@heroui/button'
import { ExclamationCircleIcon } from '@heroicons/react/24/outline'

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  useEffect(() => {
    console.error(error)
  }, [error])

  return (
    <div className="h-full flex items-center justify-center px-6">
      <div className="flex flex-col items-center text-center max-w-md gap-6">
        <div className="rounded-full bg-danger-100 p-6 text-danger">
          <ExclamationCircleIcon className="size-12" />
        </div>

        <h2 className="text-2xl font-semibold text-default-800">
          Something went wrong!
        </h2>

        <p className="text-default-500">
          An unexpected error occurred. Please try again.
        </p>

        <Button
          color="primary"
          radius="full"
          size="lg"
          onPress={
            // Attempt to recover by trying to re-render the segment
            () => reset()
          }
        >
          Try again
        </Button>
      </div>
    </div>
  )
}
