interface RitualLogoProps {
  className?: string;
}

export function RitualLogo({ className }: RitualLogoProps) {
  return (
    <svg
      viewBox="0 0 512 512"
      aria-hidden="true"
      className={className}
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <defs>
        <linearGradient id="ritual-logo-metal" x1="88" y1="88" x2="424" y2="424" gradientUnits="userSpaceOnUse">
          <stop offset="0" stopColor="#f3efea" />
          <stop offset="1" stopColor="#c7c1bb" />
        </linearGradient>
        <linearGradient id="ritual-logo-r" x1="222" y1="210" x2="322" y2="330" gradientUnits="userSpaceOnUse">
          <stop offset="0" stopColor="#b91c1c" />
          <stop offset="1" stopColor="#e5e7eb" />
        </linearGradient>
      </defs>

      <circle cx="256" cy="256" r="170" stroke="url(#ritual-logo-metal)" strokeWidth="6" />
      <path d="M78 108C124 66 190 44 256 44C322 44 388 66 434 108" stroke="#b91c1c" strokeWidth="5" strokeLinecap="round" />
      <path d="M78 404C124 446 190 468 256 468C322 468 388 446 434 404" stroke="#b91c1c" strokeWidth="5" strokeLinecap="round" />

      <line x1="256" y1="40" x2="256" y2="102" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />
      <line x1="256" y1="410" x2="256" y2="472" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />
      <line x1="42" y1="256" x2="104" y2="256" stroke="#b91c1c" strokeWidth="5" strokeLinecap="round" />
      <line x1="408" y1="256" x2="470" y2="256" stroke="#b91c1c" strokeWidth="5" strokeLinecap="round" />

      <circle cx="20" cy="256" r="4" fill="#b91c1c" />
      <circle cx="492" cy="256" r="4" fill="#b91c1c" />
      <circle cx="102" cy="256" r="10" stroke="url(#ritual-logo-metal)" strokeWidth="5" />
      <circle cx="410" cy="256" r="10" stroke="url(#ritual-logo-metal)" strokeWidth="5" />
      <circle cx="126" cy="158" r="2.5" fill="#b91c1c" />
      <circle cx="386" cy="158" r="2.5" fill="#b91c1c" />

      <path d="M256 102L110 350H402L256 102Z" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinejoin="round" />
      <line x1="70" y1="256" x2="186" y2="256" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />
      <line x1="326" y1="256" x2="442" y2="256" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />
      <line x1="126" y1="350" x2="214" y2="350" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />
      <line x1="298" y1="350" x2="386" y2="350" stroke="url(#ritual-logo-metal)" strokeWidth="5" strokeLinecap="round" />

      <circle cx="256" cy="174" r="11" stroke="#b91c1c" strokeWidth="5" />

      <path d="M220 204V354L236 326V278H270L300 324H323L290 274C304 268 314 252 314 236C314 214 296 204 272 204H220ZM236 220H270C286 220 297 226 297 238C297 252 286 262 269 262H236V220Z" fill="url(#ritual-logo-r)" />

      <circle cx="256" cy="430" r="3.2" fill="#b91c1c" />
      <circle cx="256" cy="448" r="3.2" fill="#b91c1c" />
      <circle cx="256" cy="470" r="9" stroke="#b91c1c" strokeWidth="5" />
      <path d="M244 495L256 508L268 495" stroke="#b91c1c" strokeWidth="5" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
}
