export type Gender = 0 | 1 | 2 | 3 | 4;

export type UserProfile = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  gender: Gender;
  age?: number;
  height?: number;
  weight?: number;
  clothingSize?: string;
  preferredStyle?: string;
  budgetMin?: number;
  budgetMax?: number;
};

export type AuthResponse = {
  token: string;
  expiresAtUtc: string;
  user: UserProfile;
};

export type OtpResponse = {
  email: string;
  expiresAtUtc: string;
  developmentOtp?: string;
};

export type AdminDashboard = {
  totalUsers: number;
  totalPhotos: number;
  totalAnalyses: number;
  totalOutfits: number;
  savedOutfits: number;
  generatedToday: number;
  recentUsers: AdminUserActivity[];
};

export type AdminUserActivity = {
  userId: string;
  name: string;
  email: string;
  gender: string;
  age?: number;
  height?: number;
  weight?: number;
  clothingSize?: string;
  preferredStyle?: string;
  budgetMin?: number;
  budgetMax?: number;
  createdAtUtc: string;
  photos: number;
  outfits: number;
  savedOutfits: number;
};

export type PhotoUploadResponse = {
  photoId: string;
  fileName: string;
  blobUrl: string;
  sizeBytes: number;
  createdAtUtc: string;
};

export type PhotoAnalysis = {
  id: string;
  photoId: string;
  bodyType: string;
  skinTone: string;
  style: string;
  recommendedColors: string[];
  recommendations: string[];
  recommendedCategories: string[];
};

export type Occasion = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8;

export type Product = {
  id: string;
  productName: string;
  brand: string;
  price: number;
  rating: number;
  purchaseLink: string;
  productImage: string;
  retailer: string;
  category: string;
};

export type OutfitItem = {
  id: string;
  category: string;
  name: string;
  color: string;
  estimatedPrice: number;
  notes: string;
  product?: Product;
};

export type Outfit = {
  id: string;
  name: string;
  occasion: Occasion;
  budget: number;
  weather: string;
  stylePreference: string;
  estimatedCost: number;
  stylingExplanation: string;
  generatedImageUrl?: string;
  items: OutfitItem[];
  isSaved: boolean;
};
