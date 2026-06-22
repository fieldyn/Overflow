'use client';
import { HeroUIProvider } from "@heroui/react";
import { useRouter } from "next/navigation";
import { ReactNode } from "react";

export default function Providers({ children }: { children: ReactNode }) {
  const router = useRouter();
  // 2. Wrap HeroUIProvider at the root of your app
  return (
    <HeroUIProvider navigate={router.push} className="h-full flex flex-col">
      {children}
    </HeroUIProvider>
  );
}