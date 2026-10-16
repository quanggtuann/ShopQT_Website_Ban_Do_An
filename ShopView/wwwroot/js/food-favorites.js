(function () {
    const form = document.getElementById('foodFavoriteForm');
    const feedback = document.getElementById('foodFavoriteFeedback');
    if (!form || !feedback) return;

    const pendingFoodIds = new Set();
    let feedbackTimer;

    function showFeedback(message, isError) {
        clearTimeout(feedbackTimer);
        feedback.textContent = message;
        feedback.classList.toggle('error', isError);
        feedback.hidden = false;
        feedbackTimer = setTimeout(() => { feedback.hidden = true; }, 3000);
    }

    function goToLogin() {
        const url = new URL(form.dataset.loginUrl, window.location.origin);
        url.searchParams.set('returnUrl', window.location.pathname + window.location.search);
        window.location.assign(url.toString());
    }

    document.addEventListener('keydown', function (event) {
        const link = event.target.closest('a.js-favorite-toggle');
        if (link && event.key === ' ') {
            event.preventDefault();
            link.click();
        }
    });

    document.addEventListener('click', async function (event) {
        const link = event.target.closest('a.js-favorite-toggle');
        if (!link) return;

        event.preventDefault();
        const foodId = link.dataset.foodId;
        if (!foodId || pendingFoodIds.has(foodId)) return;

        const matchingLinks = Array.from(document.querySelectorAll('a.js-favorite-toggle'))
            .filter(item => item.dataset.foodId === foodId);
        pendingFoodIds.add(foodId);
        matchingLinks.forEach(item => item.setAttribute('aria-disabled', 'true'));

        try {
            const body = new FormData(form);
            body.set('id', foodId);
            const response = await fetch(form.action, {
                method: 'POST',
                body,
                credentials: 'same-origin',
                headers: { Accept: 'application/json' }
            });

            if (response.redirected) {
                const redirectPath = new URL(response.url).pathname.toLowerCase();
                if (redirectPath.startsWith('/account/login')) goToLogin();
                else window.location.assign(response.url);
                return;
            }
            if (response.status === 401) {
                goToLogin();
                return;
            }

            const result = await response.json();
            if (!response.ok || result.success !== true || typeof result.isFavorite !== 'boolean') {
                throw new Error('Favorite update failed');
            }

            matchingLinks.forEach(item => {
                item.classList.toggle('active', result.isFavorite);
                item.setAttribute('aria-pressed', String(result.isFavorite));
                const icon = item.querySelector('i');
                if (icon) {
                    icon.classList.toggle('fa-heart', result.isFavorite);
                    icon.classList.toggle('fa-heart-o', !result.isFavorite);
                }
            });
            showFeedback(result.isFavorite ? form.dataset.addedMessage : form.dataset.removedMessage, false);
        } catch {
            showFeedback(form.dataset.errorMessage, true);
        } finally {
            pendingFoodIds.delete(foodId);
            matchingLinks.forEach(item => item.removeAttribute('aria-disabled'));
        }
    });
})();
