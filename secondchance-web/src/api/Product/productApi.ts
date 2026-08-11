import axiosClient from "../axiosClient";

export type ProductCondition = "BrandNew" | "LikeNew" | "GentlyUsed" | "WellUsed";

export interface Product {
  id: string;
  title: string;
  description: string;
  price: number;
  condition: ProductCondition; 
  categoryId: string;
  sellerId: string; 
  isAvailable?: boolean;
  images?: ProductImageResponse[];
}

export interface UpdateProductDto {
  title?: string;
  description?: string;
  price?: number;
  condition?: string;
  categoryId?: string;
}

export interface ProductImageResponse{
  id: string;
  imageUrl: string;
  isMain: boolean;
  productId: string;
}


export const productApi = {
  getAllProducts: async (): Promise<Product[]> => {
    const response = await axiosClient.get<any>('/Product');
    
    if (Array.isArray(response.data)) {
      return response.data;
    }
    return response.data.items || response.data.data || [];
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
  },

  updateProduct: async(productId: string, updatedData: UpdateProductDto) => {
    const response = await axiosClient.put(`/Product/${productId}`, {updatedData});
    return response.data;
  },

  uploadProductImage: async(productId: string, file: File): Promise<ProductImageResponse> => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await axiosClient.post<ProductImageResponse>(
      `/Product/${productId}/images`,
      formData,{
        headers:{
          'Content-Type':'multipart/form-data',
        },
      }
    );
    return response.data;
  },

  getProductById: async(id: string): Promise<Product> => {
      const response = await axiosClient.get<Product>(`/Product/${id}`);
      return response.data;
  },

  getMyListingProducts:  async(): Promise<Product[]> => {
    const response = await axiosClient.get<Product[]>(`/Product/my-listing`);
    return response.data;
  }
};