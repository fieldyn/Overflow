'use client'
import Link from "next/link";
import { forwardRef, ComponentPropsWithoutRef } from "react";

const LinkComponent = forwardRef<
  HTMLAnchorElement,                      // el nodo DOM real: el <a>
  ComponentPropsWithoutRef<typeof Link>   // las props de next/link (href, prefetch...)
>((props, ref) => {
  return <Link ref={ref} {...props} />;
});

LinkComponent.displayName = "LinkComponent";

export default LinkComponent;