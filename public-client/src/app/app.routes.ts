import { CanActivateFn, Routes } from '@angular/router';
import { inject } from '@angular/core';
import Keycloak from 'keycloak-js';
import { About } from './about/about';
import { Profile } from './profile/profile';
import { User } from './user/user';
import { CreateUser } from './create-user/create-user';

export const authGuard: CanActivateFn = () => {
  const keycloak = inject(Keycloak);
  if (keycloak.authenticated) return true;
  keycloak.login({ redirectUri: window.location.href });
  return false;
};

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/about',
    pathMatch: 'full',
  },
  {
    path: 'about',
    component: About,
  },
  {
    path: 'profile',
    component: Profile,
    canActivate: [authGuard],
  },
  {
    path: 'user',
    component: User,
    canActivate: [authGuard],
  },
  {
    path: 'create-user',
    component: CreateUser,
    canActivate: [authGuard],
  },
];
