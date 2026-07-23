import axiosClient from "../axiosClient";

export type ProductCondition = "BrandNew" | "LikeNew" | "GentlyUsed" | "WellUsed";
export type ProductStatus = "Unavailable" | "Available" | "Reserved" | "Sold";

export interface Product {
  id: string;
  title: string;
  description: string;
  price: number;
  condition: ProductCondition; 
  categoryId: string;
  sellerId: string; 
  status: ProductStatus;
  isAvailable?: boolean;
}


export const productApi = {
  getAllProducts: async (): Promise<Product[]> => {
    const response = await axiosClient.get<any>('/Product');
    return response.data.data || [];
  },

  freezeProduct: async (id: string, reason: string): Promise<void> => {
    await axiosClient.put(`/admin/products/${id}/freeze?reason=${encodeURIComponent(reason)}`);
  },

  unFreezeProduct: async(id: string): Promise<void> => {
    await axiosClient.put(`/admin/products/${id}/unfreeze`)
  },

  createProduct: async(productData: Omit<Product, 'id' | 'status'>): Promise<Product> => {
    const response = await axiosClient.post<{data: Product}>('Product/create-product', productData);
    if(!response.data.data){
      throw new Error("Failed to create product: No data returned from server");
    }
    return response.data.data
  }
};