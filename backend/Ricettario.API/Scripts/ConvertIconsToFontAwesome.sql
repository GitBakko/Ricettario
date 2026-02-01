-- Script to convert Bootstrap Icons to Font Awesome in the database
-- Run this script to update existing records

-- Update RecipePhaseTypes icons
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-basket-shopping' WHERE Icon = 'bi-basket' OR Icon = 'bi-basket-fill';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-compact-disc' WHERE Icon = 'bi-vinyl';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-clock-rotate-left' WHERE Icon = 'bi-clock-history';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-hourglass-half' WHERE Icon = 'bi-hourglass-split' OR Icon = 'bi-hourglass';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-border-all' WHERE Icon = 'bi-grid-3x3' OR Icon = 'bi-grid-3x3-gap-fill';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-virus' WHERE Icon = 'bi-virus';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-fire' WHERE Icon = 'bi-fire';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-layer-group' WHERE Icon = 'bi-layers' OR Icon = 'bi-layers-fill';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-box' WHERE Icon = 'bi-box-seam';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-rotate-right' WHERE Icon = 'bi-arrow-down-right-circle';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-droplet' WHERE Icon = 'bi-droplet-half' OR Icon = 'bi-droplet';
UPDATE RecipePhaseTypes SET Icon = 'fa-solid fa-circle' WHERE Icon = 'bi-circle';

-- Update Categories icons
UPDATE Categories SET Icon = 'fa-solid fa-pizza-slice' WHERE Icon = 'bi-pie-chart-fill';
UPDATE Categories SET Icon = 'fa-solid fa-bread-slice' WHERE Icon = 'bi-basket-fill';
UPDATE Categories SET Icon = 'fa-solid fa-border-all' WHERE Icon = 'bi-grid-3x3-gap-fill';
UPDATE Categories SET Icon = 'fa-solid fa-cake-candles' WHERE Icon = 'bi-cake2-fill';
UPDATE Categories SET Icon = 'fa-solid fa-star' WHERE Icon = 'bi-star-fill';
UPDATE Categories SET Icon = 'fa-solid fa-moon' WHERE Icon = 'bi-moon-fill';
UPDATE Categories SET Icon = 'fa-solid fa-cookie' WHERE Icon = 'bi-cookie';
UPDATE Categories SET Icon = 'fa-solid fa-hexagon' WHERE Icon = 'bi-hexagon-fill';
UPDATE Categories SET Icon = 'fa-solid fa-layer-group' WHERE Icon = 'bi-layers-fill';
UPDATE Categories SET Icon = 'fa-solid fa-diamond' WHERE Icon = 'bi-suit-diamond-fill';
UPDATE Categories SET Icon = 'fa-solid fa-ellipsis' WHERE Icon = 'bi-three-dots';
UPDATE Categories SET Icon = 'fa-solid fa-folder' WHERE Icon = 'bi-folder';

-- Generic conversion for any remaining bi- icons
UPDATE RecipePhaseTypes SET Icon = REPLACE(Icon, 'bi-', 'fa-solid fa-') WHERE Icon LIKE 'bi-%';
UPDATE Categories SET Icon = REPLACE(Icon, 'bi-', 'fa-solid fa-') WHERE Icon LIKE 'bi-%';
