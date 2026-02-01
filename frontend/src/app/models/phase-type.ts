export interface PhaseType {
    id: number;
    name: string;
    description?: string;
    
    icon?: string;
    color?: string;

    isActiveWork: boolean;
    isSystemDefault: boolean;
}
