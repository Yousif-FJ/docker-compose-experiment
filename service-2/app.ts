import { createServer } from 'http';
import { getSystemInformation } from './utils.js';

const server = createServer(async (req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'application/json');
  res.end(JSON.stringify(await getSystemInformation()));
});

const PORT = 3000;
server.listen(PORT, async () => {
  console.log(`Server running at http://localhost:${PORT}/`);
});
