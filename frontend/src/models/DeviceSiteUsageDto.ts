export interface DeviceSiteUsageDto {
  deviceName: string;
  siteUsage: SiteUsage[];
}

export interface SiteUsage {
  siteName: string;
  readUsage: number;
  writeUsage: number;
}
