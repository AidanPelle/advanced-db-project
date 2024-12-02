export interface WriteMatrixDto {
    siteId: number;
    siteName: string;
    frequencies: WriteMatrixDeviceDto[];
}

export interface WriteMatrixDeviceDto {
    deviceId: string;
    deviceName: string;
    frequencyValue: number;
}