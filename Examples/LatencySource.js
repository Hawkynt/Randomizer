async function getNetworkLatencySeed() {
  return await _getNetworkLatencySeed(
    [
      'https://google.com',
      'https://bing.com',
      'https://yahoo.com',
      'https://duckduckgo.com',
      'https://ecosia.org',
    ], 
    10
  );
}

async function _getNetworkLatencySeed(hosts, count) {
  let seed = 0;

  const getLatency = (host) =>
    new Promise((resolve) => {
      const start = Date.now();
      fetch(host, { mode: 'no-cors' })
        .then(() => resolve(Date.now() - start))
        .catch(() => resolve(Date.now() - start)); // resolve even if the fetch fails
    });

  const combineSeed = (seed, latency) => (seed * 31 + latency) % Number.MAX_SAFE_INTEGER;

  for (let i = 0; i < count; i++) {
    const latency = await getLatency(hosts[i % hosts.length]);
    seed = combineSeed(seed, latency);
  }

  return seed;
}
