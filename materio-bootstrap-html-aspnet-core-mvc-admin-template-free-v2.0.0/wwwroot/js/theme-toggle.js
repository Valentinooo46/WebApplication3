(function () {
  const storageKey = 'preferredTheme';
  const darkClass = 'dark-style';
  const lightClass = 'light-style';

  function applyTheme(theme) {
    const root = document.documentElement;
    if (!root) {
      return;
    }

    if (theme === 'dark') {
      root.classList.remove(lightClass);
      root.classList.add(darkClass);
    } else {
      root.classList.remove(darkClass);
      if (!root.classList.contains(lightClass)) {
        root.classList.add(lightClass);
      }
    }

    updateIcon(theme);
  }

  function updateIcon(theme) {
    const icon = document.querySelector('[data-theme-toggle-icon]');
    if (!icon) {
      return;
    }

    icon.classList.remove('ri-sun-line', 'ri-moon-clear-line');
    icon.classList.add(theme === 'dark' ? 'ri-sun-line' : 'ri-moon-clear-line');
  }

  function getCurrentTheme() {
    return document.documentElement.classList.contains(darkClass) ? 'dark' : 'light';
  }

  function toggleTheme() {
    const nextTheme = getCurrentTheme() === 'dark' ? 'light' : 'dark';
    try {
      localStorage.setItem(storageKey, nextTheme);
    } catch (e) {
      console.warn('Unable to persist theme preference', e);
    }
    applyTheme(nextTheme);
  }

  document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = (function () {
      try {
        return localStorage.getItem(storageKey);
      } catch {
        return null;
      }
    })();

    applyTheme(savedTheme === 'dark' ? 'dark' : 'light');

    const toggles = document.querySelectorAll('[data-theme-toggle]');
    toggles.forEach(btn => btn.addEventListener('click', toggleTheme));
  });
})();
