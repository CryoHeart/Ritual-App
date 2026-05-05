interface RitualWordmarkProps {
  className?: string;
}

export function RitualWordmark({ className = '' }: RitualWordmarkProps) {
  return (
    <svg
      viewBox="0 0 320 56"
      role="img"
      aria-label="RITUAL"
      className={className}
      xmlns="http://www.w3.org/2000/svg"
    >
      <defs>
        <linearGradient id="ritual-wordmark-metal" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" stopColor="#f3efea" />
          <stop offset="100%" stopColor="#c7c1bb" />
        </linearGradient>
      </defs>

      <line x1="6" y1="28" x2="20" y2="28" stroke="#7f1d1d" strokeWidth="1.7" strokeLinecap="round" />
      <circle cx="24" cy="28" r="1.8" fill="#ef4444" />
      <circle cx="30" cy="28" r="1.6" fill="#7f1d1d" />

      <text
        x="160"
        y="40"
        textAnchor="middle"
        fill="url(#ritual-wordmark-metal)"
        fontSize="40"
        fontWeight="300"
        letterSpacing="12"
        style={{ fontFamily: '"Segoe UI", "Trebuchet MS", sans-serif' }}
      >
        RITUAL
      </text>

      <circle cx="246" cy="38" r="1.9" fill="#ef4444" />

      <circle cx="290" cy="28" r="1.6" fill="#7f1d1d" />
      <circle cx="296" cy="28" r="1.8" fill="#ef4444" />
      <line x1="300" y1="28" x2="314" y2="28" stroke="#7f1d1d" strokeWidth="1.7" strokeLinecap="round" />
    </svg>
  );
}