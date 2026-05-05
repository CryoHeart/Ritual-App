import { useLayoutEffect, useRef } from 'react';

interface Props {
  text: string;
  className?: string;
  minPx?: number;
  maxPx?: number;
}

/**
 * Renders text at the largest font size that still fits in one line
 * without overflowing its container. Uses ResizeObserver so it
 * re-evaluates whenever the container is resized.
 */
export function FitText({ text, className, minPx = 18, maxPx = 128 }: Props) {
  const ref = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    const el = ref.current;
    if (!el) return;

    const fit = () => {
      // Start at max and binary-search down to find the largest size that fits
      let lo = minPx;
      let hi = maxPx;

      // Quick check: does max size fit already?
      el.style.fontSize = `${hi}px`;
      if (el.scrollWidth <= el.clientWidth) return;

      while (hi - lo > 1) {
        const mid = Math.round((lo + hi) / 2);
        el.style.fontSize = `${mid}px`;
        if (el.scrollWidth <= el.clientWidth) {
          lo = mid;
        } else {
          hi = mid;
        }
      }
      el.style.fontSize = `${lo}px`;
    };

    fit();

    const ro = new ResizeObserver(fit);
    ro.observe(el);
    return () => ro.disconnect();
  }, [text, minPx, maxPx]);

  return (
    <div
      ref={ref}
      className={`overflow-hidden whitespace-nowrap ${className ?? ''}`}
    >
      {text}
    </div>
  );
}
