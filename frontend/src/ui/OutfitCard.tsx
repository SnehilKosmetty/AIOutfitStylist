import { ExternalLink, Heart, Trash2 } from 'lucide-react';
import { toAbsoluteUrl } from '../lib/api';
import type { Outfit } from '../types';

type Props = {
  outfit: Outfit;
  onSave?: (id: string) => void;
  onDelete?: (id: string) => void;
};

export function OutfitCard({ outfit, onSave, onDelete }: Props) {
  const imageUrl = outfit.generatedImageUrl?.includes('placehold.co') ? undefined : toAbsoluteUrl(outfit.generatedImageUrl);

  return (
    <article className="flex overflow-hidden rounded-md border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900 flex-col">
      {imageUrl ? (
        <div className="grid h-[520px] shrink-0 place-items-center border-b border-slate-200 bg-slate-100 dark:border-slate-800 dark:bg-slate-950">
          <img src={imageUrl} alt={outfit.name} className="h-full w-full object-contain" />
        </div>
      ) : (
        <div className="shrink-0 border-b border-slate-200 bg-slate-50 p-4 dark:border-slate-800 dark:bg-slate-950">
          <div className="flex items-center justify-between gap-3">
            <div>
              <p className="text-xs uppercase tracking-wider text-teal-700 dark:text-teal-300">Recommendation</p>
              <h3 className="mt-1 text-lg font-semibold">{outfit.name}</h3>
            </div>
            <span className="rounded-md bg-gold/15 px-2 py-1 text-sm font-semibold text-slate-900 dark:text-gold">${outfit.estimatedCost.toFixed(2)}</span>
          </div>
          <div className="mt-3 flex h-2 overflow-hidden rounded-full">
            {outfit.items.slice(0, 4).map((item) => (
              <span key={item.id} className="flex-1" style={{ backgroundColor: colorToCss(item.color) }} />
            ))}
          </div>
          <AvatarPreview outfit={outfit} />
        </div>
      )}
      <div className="relative z-10 space-y-4 bg-white p-4 dark:bg-slate-900">
        {imageUrl && (
          <div className="flex items-start justify-between gap-4">
            <div>
              <h3 className="text-lg font-semibold">{outfit.name}</h3>
              <p className="text-sm text-slate-500 dark:text-slate-400">{outfit.weather} - {outfit.stylePreference}</p>
            </div>
            <span className="rounded-md bg-gold/15 px-2 py-1 text-sm font-semibold text-slate-900 dark:text-gold">${outfit.estimatedCost.toFixed(2)}</span>
          </div>
        )}
        {!imageUrl && <p className="text-sm text-slate-500 dark:text-slate-400">{outfit.weather} - {outfit.stylePreference}</p>}
        <p className="text-sm leading-6 text-slate-600 dark:text-slate-300">{outfit.stylingExplanation}</p>
        <div className="space-y-2">
          {outfit.items.map((item) => (
            <div key={item.id} className="flex items-center justify-between gap-3 rounded-md bg-slate-50 p-3 text-sm dark:bg-slate-950">
              <div>
                <p className="font-medium">{item.category}: {item.name}</p>
                <p className="text-slate-500">{item.color} - ${item.estimatedPrice.toFixed(2)}</p>
              </div>
              {item.product?.purchaseLink && (
                <a className="button-secondary px-2 py-2" href={normalizePurchaseLink(item.product.purchaseLink, item.name, item.color, item.category, item.estimatedPrice)} target="_blank" rel="noreferrer" aria-label="Open purchase link">
                  <ExternalLink size={16} />
                </a>
              )}
            </div>
          ))}
        </div>
        <div className="flex gap-2">
          {onSave && <button className="button" onClick={() => onSave(outfit.id)}><Heart size={16} /> Save Outfit</button>}
          {onDelete && <button className="button-secondary" onClick={() => onDelete(outfit.id)}><Trash2 size={16} /> Delete</button>}
        </div>
      </div>
    </article>
  );
}

function normalizePurchaseLink(link: string, productName: string, color: string, category: string, price: number) {
  const query = `${shortItemName(productName)} ${normalizeColor(color)} ${normalizeCategory(category)} under $${Math.ceil(price)}`;
  const encoded = encodeURIComponent(query);

  if (link.includes('oldnavy.gap.com')) {
    return `https://oldnavy.gap.com/browse/search.do?searchText=${encoded}`;
  }

  if (link.includes('gap.com')) {
    return `https://www.gap.com/browse/search.do?searchText=${encoded}`;
  }

  if (link.includes('hm.com')) {
    return `https://www2.hm.com/en_us/search-results.html?q=${encoded}`;
  }

  if (link.includes('target.com')) {
    return `https://www.target.com/s?searchTerm=${encoded}`;
  }

  if (link.includes('walmart.com')) {
    return `https://www.walmart.com/search?q=${encoded}`;
  }

  if (link.includes('amazon.com')) {
    return `https://www.amazon.com/s?k=${encoded}`;
  }

  if (link.includes('macys.com')) {
    return `https://www.macys.com/shop/featured/${encoded}`;
  }

  if (link.includes('nordstromrack.com')) {
    return `https://www.nordstromrack.com/search?keyword=${encoded}`;
  }

  if (link.includes('zara.com')) {
    return `https://www.zara.com/us/en/search?searchTerm=${encoded}`;
  }

  if (link.includes('asos.com')) {
    return `https://www.asos.com/us/search/?q=${encoded}`;
  }

  return link;
}

function shortItemName(name: string) {
  return name
    .replace(/^Men's\s+/i, '')
    .replace(/^Women's\s+/i, '')
    .replace(/^Unisex\s+/i, '');
}

function AvatarPreview({ outfit }: { outfit: Outfit }) {
  const top = outfit.items.find((item) => item.category === 'Shirt/Top');
  const bottom = outfit.items.find((item) => item.category === 'Bottom');
  const shoes = outfit.items.find((item) => item.category === 'Shoes');
  const accessories = outfit.items.find((item) => item.category === 'Accessories');

  return (
    <div className="mt-4 grid h-80 place-items-center rounded-md bg-gradient-to-b from-slate-100 to-white p-4 dark:from-slate-900 dark:to-slate-950">
      <div className="relative h-72 w-44">
        <div className="absolute left-1/2 top-0 h-12 w-10 -translate-x-1/2 rounded-full bg-slate-300 dark:bg-slate-200" />
        <div className="absolute left-1/2 top-14 h-20 w-24 -translate-x-1/2 rounded-t-3xl rounded-b-lg border border-black/10" style={{ backgroundColor: colorToCss(top?.color ?? 'teal') }} />
        <div className="absolute left-[21px] top-17 h-24 w-7 -rotate-6 rounded-full border border-black/10" style={{ backgroundColor: colorToCss(top?.color ?? 'teal') }} />
        <div className="absolute right-[21px] top-17 h-24 w-7 rotate-6 rounded-full border border-black/10" style={{ backgroundColor: colorToCss(top?.color ?? 'teal') }} />
        <div className="absolute left-[52px] top-[130px] h-28 w-9 rounded-b-2xl border border-black/10" style={{ backgroundColor: colorToCss(bottom?.color ?? 'navy') }} />
        <div className="absolute right-[52px] top-[130px] h-28 w-9 rounded-b-2xl border border-black/10" style={{ backgroundColor: colorToCss(bottom?.color ?? 'navy') }} />
        <div className="absolute left-[43px] top-[258px] h-5 w-14 rounded-full border border-black/10" style={{ backgroundColor: colorToCss(shoes?.color ?? 'white') }} />
        <div className="absolute right-[43px] top-[258px] h-5 w-14 rounded-full border border-black/10" style={{ backgroundColor: colorToCss(shoes?.color ?? 'white') }} />
        <div className="absolute left-1/2 top-[118px] h-3 w-28 -translate-x-1/2 rounded-full" style={{ backgroundColor: colorToCss(accessories?.color ?? 'brown') }} />
      </div>
      <p className="sr-only">Avatar preview for {outfit.name}</p>
    </div>
  );
}

function normalizeCategory(category: string) {
  if (category === 'Shirt/Top') return 'shirt';
  if (category === 'Bottom') return 'pants';
  if (category === 'Shoes') return 'shoes';
  if (category === 'Accessories') return 'accessories';
  return category;
}

function normalizeColor(color: string) {
  return color
    .replace(/earth tones/i, 'brown')
    .replace(/muted blues/i, 'blue')
    .replace(/grays/i, 'gray')
    .toLowerCase();
}

function colorToCss(color: string) {
  const value = color.toLowerCase();
  if (value.includes('beige')) return '#d8c3a5';
  if (value.includes('blue')) return '#93b7d8';
  if (value.includes('white')) return '#f8fafc';
  if (value.includes('olive')) return '#708238';
  if (value.includes('earth')) return '#9a6b4f';
  if (value.includes('mustard')) return '#d4a72c';
  if (value.includes('navy')) return '#1f3a5f';
  if (value.includes('cream')) return '#f3ead7';
  return '#0f766e';
}
