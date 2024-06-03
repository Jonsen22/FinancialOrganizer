/** @type {import('next').NextConfig} */
const nextConfig = {
  webpackDevMiddleware: (config) => {
    // Enable polling to allow hot reloading in Docker volumes
    config.watchOptions = {
      poll: 1000, // Check for changes every 1 second
      aggregateTimeout: 300, // Delay before rebuilding
    };
    return config;
  },
};

export default nextConfig;