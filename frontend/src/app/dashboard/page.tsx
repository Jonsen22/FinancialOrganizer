'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '../../contexts/authContext';

const Dashboard = () => {
  const { user, logout } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!user) {
      router.push('/');
    }
  }, [user, router]);

  return (
    <div>
      <h1>Welcome to the Dashboard, {user ? user.email : 'Guest'}!</h1>
      <button onClick={logout}>Logout</button>
    </div>
  );
};

export default Dashboard;
