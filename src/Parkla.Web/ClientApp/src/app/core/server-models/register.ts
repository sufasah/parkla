export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  name: string;
  surname: string;
  phone: string;
  address: string | null;
  birthdate: string | null;
  gender: string | null;
  cityId: number | null;
  districtId: number | null;
};
