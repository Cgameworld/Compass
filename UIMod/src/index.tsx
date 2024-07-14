import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";

const register: ModRegistrar = (moduleRegistry) => {

    const Rotation$ = bindValue<number>('Compass', 'Rotation');

    const CustomMenuButton = () => {
        const RotationNum: number = useValue(Rotation$);
        return <div>
            <button id="MapTextureReplacer-MainGameButton" className="button_ke4 button_ke4 button_h9N" onClick={() => trigger("map_texture", "MainWindowCreate")}>
                {RotationNum}
            </button>
        </div>;
    }

    moduleRegistry.append('GameTopRight', CustomMenuButton);
}

export default register;