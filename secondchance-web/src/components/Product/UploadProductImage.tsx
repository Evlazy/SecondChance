import React, { useState, useRef } from 'react';
import { productApi, type ProductImageResponse } from '../../api/Product/productApi';

interface UploadProductImageModalProps {
  productId: string;
  isOpen: boolean;
  onClose: () => void;
  onImageUploaded?: (newImage: ProductImageResponse) => void;
}

export const UploadProductImageModal: React.FC<UploadProductImageModalProps> = ({
  productId,
  isOpen,
  onClose,
  onImageUploaded,
}) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isUploading, setIsUploading] = useState<boolean>(false);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [isDragOver, setIsDragOver] = useState<boolean>(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  if (!isOpen) return null;

  const handleFileSelect = (file: File) => {
    if (!file.type.startsWith('image/')) {
      setErrorMsg('Please select a valid image file (PNG, JPG, WEBP, etc.).');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setErrorMsg('Image size must be less than 5MB.');
      return;
    }

    setErrorMsg(null);
    setSelectedFile(file);
    setPreviewUrl(URL.createObjectURL(file));
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      handleFileSelect(e.target.files[0]);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFileSelect(e.dataTransfer.files[0]);
    }
  };

  const handleUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile) {
      setErrorMsg('Please select an image to upload.');
      return;
    }

    setIsUploading(true);
    setErrorMsg(null);

    try {
      const uploadedImage = await productApi.uploadProductImage(productId, selectedFile);
      
      setSelectedFile(null);
      setPreviewUrl(null);

      if (onImageUploaded) {
        onImageUploaded(uploadedImage);
      }
      
      onClose();
    } catch (err: any) {
      if (err.response?.status === 403) {
        setErrorMsg('You do not have permission to upload images for this product.');
      } else if (err.response?.status === 404) {
        setErrorMsg('Product not found.');
      } else {
        setErrorMsg(err.response?.data?.message || 'Failed to upload image. Please try again.');
      }
    } finally {
      setIsUploading(false);
    }
  };

  const handleCloseModal = () => {
    if (isUploading) return;
    setSelectedFile(null);
    setPreviewUrl(null);
    setErrorMsg(null);
    onClose();
  };

  return (
    <div style={styles.overlay}>
      <div style={styles.modal}>
        <div style={styles.header}>
          <h3>Upload Product Image</h3>
          <button style={styles.closeBtn} onClick={handleCloseModal} disabled={isUploading}>
            &times;
          </button>
        </div>

        <form onSubmit={handleUpload}>
          <div
            style={{
              ...styles.dropZone,
              borderColor: isDragOver ? '#0066cc' : '#ccc',
              backgroundColor: isDragOver ? '#f0f8ff' : '#fafafa',
            }}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
            onClick={() => fileInputRef.current?.click()}
          >
            <input
              type="file"
              ref={fileInputRef}
              style={{ display: 'none' }}
              accept="image/*"
              onChange={handleInputChange}
            />

            {previewUrl ? (
              <div style={styles.previewContainer}>
                <img src={previewUrl} alt="Preview" style={styles.previewImage} />
                <p style={styles.changeText}>Click or drag to replace image</p>
              </div>
            ) : (
              <div>
                <p style={{ margin: '0 0 8px 0', fontWeight: 'bold' }}>
                  Drag & drop your product photo here
                </p>
                <p style={{ margin: 0, color: '#666', fontSize: '0.85rem' }}>
                  or click to browse from computer
                </p>
              </div>
            )}
          </div>

          {errorMsg && <p style={styles.errorText}>{errorMsg}</p>}

          {/* Action Buttons */}
          <div style={styles.actions}>
            <button
              type="button"
              style={styles.cancelBtn}
              onClick={handleCloseModal}
              disabled={isUploading}
            >
              Cancel
            </button>
            <button
              type="submit"
              style={{
                ...styles.submitBtn,
                opacity: !selectedFile || isUploading ? 0.6 : 1,
                cursor: !selectedFile || isUploading ? 'not-allowed' : 'pointer',
              }}
              disabled={!selectedFile || isUploading}
            >
              {isUploading ? 'Uploading...' : 'Upload Image'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

const styles: { [key: string]: React.CSSProperties } = {
  overlay: {
    position: 'fixed',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    zIndex: 1000,
  },
  modal: {
    backgroundColor: '#fff',
    borderRadius: '8px',
    padding: '24px',
    width: '100%',
    maxWidth: '450px',
    boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '16px',
  },
  closeBtn: {
    background: 'none',
    border: 'none',
    fontSize: '24px',
    cursor: 'pointer',
  },
  dropZone: {
    border: '2px dashed #ccc',
    borderRadius: '6px',
    padding: '20px',
    textAlign: 'center',
    cursor: 'pointer',
    minHeight: '160px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    transition: 'all 0.2s ease',
  },
  previewContainer: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  previewImage: {
    maxHeight: '140px',
    maxWidth: '100%',
    borderRadius: '4px',
    objectFit: 'cover',
  },
  changeText: {
    fontSize: '0.75rem',
    color: '#888',
    marginTop: '6px',
    marginBottom: 0,
  },
  errorText: {
    color: '#d9534f',
    fontSize: '0.85rem',
    marginTop: '10px',
  },
  actions: {
    display: 'flex',
    justifyContent: 'flex-end',
    gap: '12px',
    marginTop: '20px',
  },
  cancelBtn: {
    padding: '8px 16px',
    border: '1px solid #ccc',
    borderRadius: '4px',
    background: '#fff',
    cursor: 'pointer',
  },
  submitBtn: {
    padding: '8px 16px',
    border: 'none',
    borderRadius: '4px',
    backgroundColor: '#0066cc',
    color: '#fff',
    fontWeight: 'bold',
  },
};