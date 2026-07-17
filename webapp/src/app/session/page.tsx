import ErrorButtons from "./ErrorButtons";

export default function SessionPage() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">Session</h1>

      <section className="flex flex-col gap-3">
        <h2 className="text-lg font-medium text-default-700">Test errors</h2>
        <p className="text-default-500">
          Trigger an error response from the API to test the error boundary.
        </p>
        <ErrorButtons />
      </section>
    </div>
  );
}
