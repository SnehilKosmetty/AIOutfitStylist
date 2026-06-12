export const genderOptions = [
  { value: 0, label: 'Not specified' },
  { value: 1, label: 'Female' },
  { value: 2, label: 'Male' },
  { value: 3, label: 'Non-binary' },
  { value: 4, label: 'Prefer not to say' }
];

export const ageOptions = Array.from({ length: 88 }, (_, index) => 13 + index);

export const heightOptions = [
  4.8, 4.9,
  5.0, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9,
  6.0, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6
];

export const weightOptions = Array.from({ length: 151 }, (_, index) => 80 + index * 2);

export const clothingSizeOptions = ['XS', 'S', 'M', 'L', 'XL', 'XXL', '3XL', '4XL'];

export const styleOptions = [
  'Casual',
  'Modern casual',
  'Smart casual',
  'Formal',
  'Business professional',
  'Streetwear',
  'Minimal',
  'Classic',
  'Bohemian',
  'Athleisure',
  'Party',
  'Wedding guest'
];

export const budgetOptions = [25, 50, 75, 100, 150, 200, 300, 500, 750, 1000];

export function passwordScore(password: string) {
  return [
    password.length >= 8,
    /[A-Z]/.test(password),
    /[a-z]/.test(password),
    /\d/.test(password),
    /[^a-zA-Z0-9]/.test(password)
  ].filter(Boolean).length;
}

export function passwordStrengthLabel(score: number) {
  if (score <= 2) return 'Weak';
  if (score <= 4) return 'Good';
  return 'Strong';
}
