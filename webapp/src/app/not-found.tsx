import { Button } from "@heroui/button";
import { ExclamationTriangleIcon } from "@heroicons/react/24/outline";
import LinkComponent from "@/components/LinkComponent";

export default function NotFound() {
  return (
    <div className="h-full flex items-center justify-center px-6">
      <div className="flex flex-col items-center text-center max-w-md gap-6">
        <div className="rounded-full bg-warning-100 p-6 text-warning">
          <ExclamationTriangleIcon className="size-12" />
        </div>

        <h1 className="text-7xl font-extrabold tracking-tight text-default-900">
          404
        </h1>

        <h2 className="text-2xl font-semibold text-default-800">
          Page Not Found
        </h2>

        <p className="text-default-500">
          Sorry, the page you are looking for doesn&apos;t exist.
        </p>

        <Button as={LinkComponent} href="/" color="primary" radius="full" size="lg">
          Back to Home
        </Button>
      </div>
    </div>
  );
}