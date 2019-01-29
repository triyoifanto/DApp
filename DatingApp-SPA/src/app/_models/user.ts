import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lokingFor?: string;
    photos?: Photo[];
}
// for optional type consider to placed after the required prop.