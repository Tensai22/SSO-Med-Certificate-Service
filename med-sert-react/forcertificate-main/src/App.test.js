import { render, screen } from '@testing-library/react';

jest.mock('axios', () => {
  const mockClient = {
    get: jest.fn(),
    post: jest.fn(),
    put: jest.fn(),
    delete: jest.fn(),
    interceptors: {
      request: { use: jest.fn() },
      response: { use: jest.fn() },
    },
  };

  return {
    create: jest.fn(() => mockClient),
  };
});

const App = require('./App').default;

test('renders login form for unauthenticated user', () => {
  render(<App />);
  expect(screen.getByPlaceholderText('Логин')).toBeInTheDocument();
});
