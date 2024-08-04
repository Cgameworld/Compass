import React from 'react';
import engine from 'cohtml/cohtml';

interface ButtonModProps {
    active: boolean;
    styles?: React.CSSProperties;
    onClick?: () => void;
    text?: string;
    inactiveText?: string;
}

const ButtonMod: React.FC<ButtonModProps> = ({ active, styles = {}, onClick = () => { }, text = "ACTIVE", inactiveText = "INACTIVE" }) => {
    const handleClick = () => {
        onClick();
        engine.trigger("audio.playSound", "select-item", 1);
    };

    return (
        <button
            className="button_WWa button_SH8"
            style={{
                backgroundColor: active ? 'var(--selectedColor)' : 'var(--menuHoverColorBright)',
                color: active ? 'white' : 'var(--menuText1Normal)',
                ...styles
            }}
            onClick={handleClick}
        >
            {active ? text : inactiveText}
        </button>
    );
};

export default ButtonMod;
